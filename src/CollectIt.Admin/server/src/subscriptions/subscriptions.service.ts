import { Injectable } from '@nestjs/common';
import { InjectModel } from '@nestjs/sequelize';
import { Subscription } from './subscriptions.model';
import { Restriction } from './restrictions/restriction.model';
import { RestrictionsService } from './restrictions/restrictions.service';
import { ResourceType } from '../common/resource-type';
import { RestrictionType } from './restrictions/restriction-type';
import CreationException from '../common/creation.exception';
import { NotFoundError } from "rxjs";

@Injectable()
export class SubscriptionsService {
  constructor(
    @InjectModel(Subscription)
    private subscriptionsRepository: typeof Subscription,
    private restrictionService: RestrictionsService,
  ) {}

  async createSubscriptionAsync(
    name: string,
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
    tags: string[] | null = null,
  ): Promise<Subscription> {
    let restriction: Restriction | null;
    let subscription: Subscription | null;
    try {
      if (restrictionType) {
        switch (restrictionType) {
          case RestrictionType.Author:
            restriction =
              await this.restrictionService.createAuthorRestrictionAsync(
                authorId,
              );
            break;
          case RestrictionType.DaysTo:
            restriction =
              await this.restrictionService.createDaysToRestrictionAsync(
                daysTo,
              );
            break;
          case RestrictionType.DaysAfter:
            restriction =
              await this.restrictionService.createDaysAfterRestrictionAsync(
                daysAfter,
              );
            break;
          case RestrictionType.Tags:
            restriction =
              await this.restrictionService.createTagsRestrictionAsync(tags);
            break;
          case RestrictionType.Size:
            restriction =
              await this.restrictionService.createSizeRestrictionAsync(
                sizeBytes,
              );
            break;
          default:
            throw new CreationException('Could not create restriction', [
              `Unsupported restriction type: ${restrictionType}`,
            ]);
        }
      }
      const errors: string[] = [];
      if (monthDuration < 1) {
        errors.push('Month duration must be positive');
      }
      if (name?.length < 6) {
        errors.push('Minimum name length is 6');
      }
      if (description?.length < 10) {
        errors.push('Minimum description length is 10');
      }
      if (price < 0) {
        errors.push('Price can not be negative');
      }
      if (maxResourcesCount < 1) {
        errors.push('Max resources count must be positive');
      }

      if (errors.length > 0) {
        throw new CreationException('Could not create subscription', errors);
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
            id: subscription.id,
          },
        });
      }
      throw e;
    }
  }

  async getSubscriptionByIdAsync(
    subscriptionId: number,
  ){
    return await this.subscriptionsRepository.findByPk(
        subscriptionId,
        {
          include: [ { all: true } ],
        },
    );
  }

  async getSubscriptionsPagedAsync(pageNumber: number, pageSize: number) {
    return await this.subscriptionsRepository.findAndCountAll({
          limit: pageSize,
          offset: (pageNumber - 1) * pageSize,
          include: [ { all: true } ]
        });
  }

  async getSubscriptionsByResourceType(
    resourceType: ResourceType,
    pageNumber: number | null,
    pageSize: number | null,
  ) {
    return await this.subscriptionsRepository.findAndCountAll({
      where: {
        appliedResourceType: resourceType,
      },
      limit: pageSize,
      offset: pageNumber ? (pageNumber - 1) * pageSize : null,
      include: [{ all: true }],
    });
  }

  async changeSubscriptionNameAsync(subscriptionId: number, name: string) {
    if (name?.length < 6) {
      throw new Error('Length of name of subscription must be greater than 6');
    }
    const x = await this.subscriptionsRepository.update(
      {
        name: name,
      },
      {
        where: {
          id: subscriptionId,
        },
      },
    );
    if (x[0] === 0) {
      throw new Error('Subscription with provided is not found');
    }
  }

  async changeSubscriptionDescriptionAsync(
    subscriptionId: number,
    description: string,
  ) {
    console.log('description', description);
    console.log('id', subscriptionId);
    if (!(description && description.length > 9)) {
      throw new Error('Length of description must be greater than 9');
    }
    const affected = await this.subscriptionsRepository.update(
      {
        description: description,
      },
      {
        where: {
          id: subscriptionId,
        },
      },
    );
    if (affected[0] === 0) {
      throw new Error('Subscription with provided id not found');
    }
  }

  async activateSubscriptionAsync(subscriptionId: number) {
    await this.setSubscriptionStateAsync(subscriptionId, true);
  }

  async deactivateSubscriptionAsync(subscriptionId: number) {
    await this.setSubscriptionStateAsync(subscriptionId, false);
  }

  private async setSubscriptionStateAsync(
    subscriptionId: number,
    isActive: boolean,
  ) {
    const affected = await this.subscriptionsRepository.update(
      {
        active: isActive,
      },
      {
        where: {
          id: subscriptionId,
        },
      },
    );
    if (affected[0] === 0) {
      throw new Error('Subscription with provided id not found');
    }
  }

  async updateSubscription(subscriptionId: number, name: string, description: string) {
    if (!Number.isInteger(subscriptionId)) throw new Error(`Subscription Id must be integer. Given: ${subscriptionId}`);
    if (!(name && description)) throw new Error(`Name or description are not provided`);
    if (name.length < 6) throw new Error(`Name length must be greater than 5. Given: ${name.length}`);
    if (description.length < 10) throw new Error(`Description length must be greater than 9. Given: ${description.length}`);
    const affected = await this.subscriptionsRepository.update({
      name: name,
      description: description
    }, {
      where: {
        id: subscriptionId
      }
    });
    if (affected[0] === 0) throw new NotFoundError('');
  }
}
