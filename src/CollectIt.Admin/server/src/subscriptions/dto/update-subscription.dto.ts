import { IsString, MinLength } from "class-validator";

export class UpdateSubscriptionDto {
    @IsString()
    @MinLength(6)
    name: string;

    @IsString()
    @MinLength(10)
    description: string;
}