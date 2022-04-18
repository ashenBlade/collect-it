import { Module } from '@nestjs/common';
import { RolesController } from './roles.controller';
import {SequelizeModule} from "@nestjs/sequelize";
import {Role} from "./roles.model";
import {User} from "../users/users.model";
import {UserRole} from "./user-role.model";

@Module({
  controllers: [RolesController],
  imports: [SequelizeModule.forFeature([Role, User, UserRole])]
})
export class RolesModule {}
