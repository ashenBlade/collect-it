import { ReadRestrictionDto } from '../restrictions/dto/read-restriction.dto';
import { Subscription } from '../subscriptions.model';
import { ResourceType } from '../../common/resource-type';
import { ToReadRestrictionDto } from '../restrictions/dto/read-restriction.dto.mapper';

export class ReadSubscriptionDto {
  readonly id: number;
  readonly name: string;
  readonly description: string;
  readonly maxResourcesCount: number;
  readonly price: number;
  readonly monthDuration: number;
  readonly appliedResourceType: ResourceType;
  readonly restriction: ReadRestrictionDto | null;
  readonly active: boolean;
}

export const ToReadSubscriptionDto = (
  s: Subscription,
): ReadSubscriptionDto => ({
  id: s.id,
  name: s.name,
  description: s.description,
  maxResourcesCount: s.maxResourcesCount,
  appliedResourceType: s.appliedResourceType,
  price: s.price,
  monthDuration: s.monthDuration,
  active: s.active,
  restriction: s.restriction ? ToReadRestrictionDto(s.restriction) : null,
});
