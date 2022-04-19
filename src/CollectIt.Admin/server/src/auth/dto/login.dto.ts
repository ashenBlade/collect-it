import {IsString} from "class-validator";

export class LoginDto {
    @IsString({
        message: 'Username must be a string'
    })
    readonly username: string;
    @IsString({
        message: 'Password must be a string'
    })
    readonly password: string;
}