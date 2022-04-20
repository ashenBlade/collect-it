export interface ReadUserDto {
    readonly id: number;
    readonly email: string;
    readonly username: string;
    readonly roles: string[];
}