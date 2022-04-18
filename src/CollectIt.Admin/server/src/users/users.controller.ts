import {Body, Controller, Delete, Get, Post, UseGuards} from '@nestjs/common';
import { CreateUserDto } from "./dto/create-user.dto";
import { UsersService } from "./users.service";
import {AuthorizeAdmin, AdminJwtAuthGuard} from "../auth/admin-jwt-auth.guard";
import {AssignRoleDto} from "./dto/assign-role.dto";
import {Authorize} from "../auth/jwt-auth.guard";
import {RemoveRoleDto} from "./dto/remove-role.dto";

@Authorize()
@Controller('api/v1/users')
export class UsersController {
    constructor(private usersService: UsersService) { }

    @Get()
    @UseGuards(AdminJwtAuthGuard)
    getAll() {
        return this.usersService.getAllUsers();
    }



    @Post(':userId/role')
    @AuthorizeAdmin()
    async assignRole(@Body() {role, userId}: AssignRoleDto) {
        await this.usersService.addRoleToUser(userId, role);
    }

    @Delete(':userId/role')
    @AuthorizeAdmin()
    async removeRole(@Body() {userId, role}: RemoveRoleDto) {
        await this.usersService.removeRoleFromUser(userId, role);

    }

}
