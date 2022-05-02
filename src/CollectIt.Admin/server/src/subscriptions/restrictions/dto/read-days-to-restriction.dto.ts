import { ReadRestrictionDto } from './read-restriction.dto';

export class ReadDaysToRestrictionDto extends ReadRestrictionDto {
  readonly daysTo: number;
}
