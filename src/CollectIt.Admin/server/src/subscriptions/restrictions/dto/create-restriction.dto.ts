import { RestrictionType } from '../restriction-type';
import { IsArray, IsInt, Min } from 'class-validator';

export class CreateRestrictionDto {
  readonly restrictionType: RestrictionType;
  @IsInt()
  readonly authorId?: number;
  @IsInt()
  @Min(1)
  readonly daysAfter?: number;
  @Min(1)
  readonly daysTo?: number;
  @Min(1)
  readonly sizeBytes?: number;
  @IsArray()
  readonly tags?: string[];
}
