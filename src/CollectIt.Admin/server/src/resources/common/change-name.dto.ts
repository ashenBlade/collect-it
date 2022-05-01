import { IsString } from 'class-validator';

export class ChangeNameDto {
  @IsString()
  readonly name: string;
}
