import { ReadRestrictionDto } from './read-restriction.dto';

export class ReadAuthorRestrictionDto extends ReadRestrictionDto {
  readonly authorId: number;
}
