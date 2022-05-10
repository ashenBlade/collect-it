import { ResourceType } from '../../common/resource-type';
import { IsInt, IsString, Min, MinLength } from 'class-validator';
import { CreateRestrictionDto } from '../restrictions/dto/create-restriction.dto';

export class CreateSubscriptionDto {
  @IsString()
  @MinLength(6)
  readonly name: string;
  @IsString()
  @MinLength(10)
  readonly description: string;
  @IsInt({
    message: 'Month duration must be integer',
  })
  @Min(1)
  readonly monthDuration: number;
  @IsInt({
    message: 'Price must be integer',
  })
  @Min(0)
  readonly price: number;
  readonly resourceType: ResourceType;
  @Min(1)
  @IsInt()
  readonly maxResourcesCount: number;
  readonly restriction: CreateRestrictionDto | null;
}
