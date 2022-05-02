import { ReadRestrictionDto } from './read-restriction.dto';

export class ReadDaysAfterRestrictionDto extends ReadRestrictionDto {
  readonly daysAfter: number;
}
