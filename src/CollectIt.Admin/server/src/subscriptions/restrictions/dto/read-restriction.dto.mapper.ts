import { ReadRestrictionDto } from './read-restriction.dto';
import { Restriction } from '../restriction.model';
import { RestrictionType } from '../restriction-type';
import { ReadAuthorRestrictionDto } from './read-author-restriction.dto';
import { ReadDaysToRestrictionDto } from './read-days-to-restriction.dto';
import { ReadDaysAfterRestrictionDto } from './read-days-after-restriction.dto';
import { ReadTagsRestrictionDto } from './read-tags-restriction.dto';
import { ReadSizeRestrictionDto } from './read-size-restriction.dto';

export const ToReadRestrictionDto = (r: Restriction): ReadRestrictionDto => {
  switch (r.restrictionType) {
    case RestrictionType.Author:
      const author: ReadAuthorRestrictionDto = {
        authorId: r.authorId,
        restrictionType: RestrictionType.Author,
      };
      return author;
    case RestrictionType.DaysTo:
      const daysTo: ReadDaysToRestrictionDto = {
        daysTo: r.daysTo,
        restrictionType: RestrictionType.DaysTo,
      };
      return daysTo;
    case RestrictionType.DaysAfter:
      const daysAfter: ReadDaysAfterRestrictionDto = {
        daysAfter: r.daysAfter,
        restrictionType: RestrictionType.DaysAfter,
      };
      return daysAfter;
    case RestrictionType.Tags:
      const tags: ReadTagsRestrictionDto = {
        tags: r.tags,
        restrictionType: RestrictionType.Tags,
      };
      return tags;
    case RestrictionType.Size:
      const size: ReadSizeRestrictionDto = {
        size: r.sizeBytes,
        restrictionType: RestrictionType.Size,
      };
      return size;
    default:
      throw new Error(`Unsupported restriction type: ${r.restrictionType}`);
  }
};
