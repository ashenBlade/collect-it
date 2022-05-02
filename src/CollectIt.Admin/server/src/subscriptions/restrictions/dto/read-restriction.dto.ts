import { RestrictionType } from '../restriction-type';

export abstract class ReadRestrictionDto {
  readonly restrictionType: RestrictionType;
}
