import { Module } from '@nestjs/common';
import { UsersService } from './users.service';
import { UsersController } from './users.controller';
import { SequelizeModule } from "@nestjs/sequelize";
import { User } from "./users.model";
import {UserRole} from "../roles/user-role.model";
import {Role} from "../roles/roles.model";

@Module({
  providers: [UsersService],
  controllers: [UsersController],
  imports: [
      SequelizeModule.forFeature([User, Role, UserRole])
  ]
})
export class UsersModule {}
