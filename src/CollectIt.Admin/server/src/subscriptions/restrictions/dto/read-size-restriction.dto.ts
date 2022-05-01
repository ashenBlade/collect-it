import { ReadRestrictionDto } from './read-restriction.dto';

export class ReadSizeRestrictionDto extends ReadRestrictionDto {
  readonly size: number;
}
