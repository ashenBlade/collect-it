import {ResourceType} from "../../common/resource-type";
import {IsInt, IsNumber, Min, MinLength} from "class-validator";

export class CreateSubscriptionDto {
    readonly name: string;
    readonly description: string;
    @IsInt({
        message: 'Month duration must be integer'
    })
    @Min(1)
    readonly monthDuration: number;
    @IsInt({
        message: 'Price must be integer'
    })
    @Min(0)
    readonly price: number;
    readonly appliedResourceType: ResourceType;
    @Min(1)
    readonly maxResourcesCount: number;
    readonly restriction: CreateSubscriptionDto | null;
    readonly active: boolean | null;
}