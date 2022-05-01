import { IsArray } from 'class-validator';
import { Type } from 'class-transformer';

export class ChangeTagsDto {
  @IsArray()
  @Type(() => String)
  readonly tags: string[];
}
