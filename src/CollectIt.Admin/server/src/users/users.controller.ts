import {Body, Controller, Get, Post, UseGuards} from '@nestjs/common';
import { CreateUserDto } from "./dto/create-user.dto";
import { UsersService } from "./users.service";
import {AuthorizeAdmin, AdminJwtAuthGuard} from "../auth/admin-jwt-auth.guard";

@Controller('api/v1/users')
export class UsersController {
    constructor(private usersService: UsersService) { }

    @Get()
    @UseGuards(AdminJwtAuthGuard)
    getAll() {
        return this.usersService.getAllUsers();
    }
}
