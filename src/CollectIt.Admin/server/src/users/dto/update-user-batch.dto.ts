import { IsEmail, IsString, MinLength } from "class-validator";

export class UpdateUserBatchDto {
    @IsEmail()
    @IsString()
    readonly email: string;

    @IsString()
    @MinLength(6)
    readonly name: string;
}