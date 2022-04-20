import {Body, Controller, Delete, Get, HttpStatus, Param, ParseIntPipe, Post, Query, Res} from '@nestjs/common';
import {UsersService} from "./users.service";
import {AuthorizeAdmin} from "../auth/admin-jwt-auth.guard";
import {AssignRoleDto} from "./dto/assign-role.dto";
import {Authorize} from "../auth/jwt-auth.guard";
import {RemoveRoleDto} from "./dto/remove-role.dto";
import {ToReadUserDto} from "./dto/read-user.dto";
import {Response} from "express";
import {NotFoundError} from "rxjs";

@Authorize()
@Controller('api/v1/users')
export class UsersController {
    constructor(private usersService: UsersService) { }

    @Get('')
    async getAll(@Query('page_number') pageNumber: number,
                 @Query('page_size') pageSize: number,
                 @Res() response: Response) {
        const result = await this.usersService.getAllUsersAsync(pageNumber, pageSize);
        const count = result.count;
        const users = result.rows;
        response.setHeader('X-Total-Count', count);
        const dtos = users.map(u => ({
            id: u.id,
            username: u.username,
            email: u.email,
            roles: u.roles?.map(r => r.name) ?? []})
        );
        response.send(dtos);
        response.end();
    }

    @Get(':userId')
    async getUserById(@Param('userId', new ParseIntPipe({errorHttpStatusCode: HttpStatus.UNPROCESSABLE_ENTITY})) userId: number, @Res() res: Response) {
        const user = await this.usersService.getUserByIdAsync(userId);
        if (!user) {
            res.status(HttpStatus.NOT_FOUND);
        } else {
            res.send(ToReadUserDto(user));
        }
        res.end();
    }

    @Get('/with-email/:email')
    async getUserByEmail(@Param('email') email: string, @Res() res: Response) {
        const user = await this.usersService.getUserByEmailAsync(email);
        if (user) {
            res.send(ToReadUserDto(user));
        } else {
            res.status(HttpStatus.NOT_FOUND);
        }
        res.end();
    }

    @Get('/with-username/:username')
    async getUserByUsername(@Param('username')username: string) {
        const user = await this.usersService.getUserByUsernameAsync(username);
        if (!user) {
            throw new NotFoundError(`User with username = ${username} not found`);
        }
        return ToReadUserDto(user);
    }

    @Post(':userId/roles')
    @AuthorizeAdmin()
    async assignRole(@Body() {role, userId}: AssignRoleDto) {
        await this.usersService.addRoleToUser(userId, role);
    }

    @Delete(':userId/roles')
    @AuthorizeAdmin()
    async removeRole(@Body() {userId, role}: RemoveRoleDto) {
        await this.usersService.removeRoleFromUser(userId, role);

    }
}
