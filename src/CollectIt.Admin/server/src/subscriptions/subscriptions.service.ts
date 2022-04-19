import { Injectable } from '@nestjs/common';
import {InjectModel} from "@nestjs/sequelize";
import {Subscription} from "./subscriptions.model";
import {Restriction} from "../restrictions/restriction.model";
import {RestrictionsService} from "../restrictions/restrictions.service";
import {ResourceType} from "../common/resource-type";
import {RestrictionType} from "../restrictions/restriction-type";

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
                                  restrictionType: RestrictionType | null,
                                  authorId: number | null,
                                  sizeBytes: number | null,
                                  daysTo: number | null,
                                  daysAfter: number | null,
                                  tags: string[] | null
                                  ): Promise<Subscription> {
        let restriction = null;
        if (restrictionType) {
            restriction = await this.restrictionService.createRestrictionAsync(restrictionType, authorId, sizeBytes, daysAfter, daysTo, tags);
        }

        const subscription = await this.subscriptionsRepository.create({
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
    }

    async getSubscriptionById(subscriptionId: number): Promise<Subscription> {
        const subscription = await this.subscriptionsRepository.findByPk(subscriptionId);
        return subscription;
    }
}
