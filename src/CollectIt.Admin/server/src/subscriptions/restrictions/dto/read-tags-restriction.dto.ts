import { ReadRestrictionDto } from './read-restriction.dto';

export class ReadTagsRestrictionDto extends ReadRestrictionDto {
  readonly tags: string[];
}
