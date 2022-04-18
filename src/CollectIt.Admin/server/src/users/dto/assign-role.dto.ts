import {IsNumber, IsString} from "class-validator";

export class AssignRoleDto {
    @IsNumber({
        allowInfinity: false,
        allowNaN: false,
    })
    readonly userId: number;

    @IsString({
        message: 'Role must be string'
    })
    readonly role: string;
}