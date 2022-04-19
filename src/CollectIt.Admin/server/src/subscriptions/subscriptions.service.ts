import {Injectable} from '@nestjs/common';
import {InjectModel} from "@nestjs/sequelize";
import {Subscription} from "./subscriptions.model";
import {Restriction} from "./restrictions/restriction.model";
import {RestrictionsService} from "./restrictions/restrictions.service";
import {ResourceType} from "../common/resource-type";
import {RestrictionType} from "./restrictions/restriction-type";

@Injectable()
export class SubscriptionsService {
    constructor(@InjectModel(Subscription) private subscriptionsRepository: typeof Subscription,
                private restrictionService: RestrictionsService) {  }

    async createSubscriptionAsync(name: string,
                                  description: string,
                                  price: number,
                                  monthDuration: number,
                                  appliedResourceType: ResourceType,
                                  maxResourcesCount: number,
                                  active: boolean | null,
                                  restrictionType: RestrictionType | null = null,
                                  authorId: number | null = null,
                                  sizeBytes: number | null = null,
                                  daysTo: number | null = null,
                                  daysAfter: number | null = null,
                                  tags: string[] | null = null): Promise<Subscription> {
        let restriction: Restriction | null = null;
        let subscription: Subscription | null = null;
        try {
            if (restrictionType) {
                switch (restrictionType) {
                    case RestrictionType.Author:
                        restriction = await this.restrictionService.createAuthorRestrictionAsync(authorId);
                        break;
                    case RestrictionType.DaysTo:
                        restriction = await this.restrictionService.createDaysToRestrictionAsync(daysTo);
                        break;
                    case RestrictionType.DaysAfter:
                        restriction = await this.restrictionService.createDaysAfterRestrictionAsync(daysAfter);
                        break;
                    case RestrictionType.Tags:
                        restriction = await this.restrictionService.createTagsRestrictionAsync(tags);
                        break;
                    case RestrictionType.Size:
                        restriction = await this.restrictionService.createSizeRestrictionAsync(sizeBytes);
                        break;
                    default:
                        throw new Error(`Unsupported restriction type: ${restrictionType}`);
                }
            }

            subscription = await this.subscriptionsRepository.create({
                name: name,
                description: description,
                maxResourcesCount: maxResourcesCount,
                price: price,
                monthDuration: monthDuration,
                appliedResourceType: appliedResourceType,
                restrictionId: restriction?.id,
                active: active ?? false,
            });
            return subscription;
        } catch (e) {
            if (restriction) {
                await this.restrictionService.deleteRestrictionById(restriction.id);
            }
            if (subscription) {
                await this.subscriptionsRepository.destroy({
                    where: {
                        id: subscription.id
                    }
                });
            }
        }
    }

    async getSubscriptionById(subscriptionId: number): Promise<Subscription> {
        const subscription = await this.subscriptionsRepository.findByPk(subscriptionId);
        return subscription;
    }

    async getSubscriptionsByResourceType(resourceType: ResourceType,
                                         pageNumber: number | null,
                                         pageSize: number | null) {
        await this.subscriptionsRepository.findAndCountAll({
            where: {
                appliedResourceType: resourceType
            },
            limit: pageSize,
            offset: pageNumber
                    ? (pageNumber - 1) * pageSize
                    : null
        })
    }
}
