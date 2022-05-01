import { Module } from '@nestjs/common';
import { RolesController } from './roles.controller';
import { SequelizeModule } from '@nestjs/sequelize';
import { Role } from './roles.model';
import { User } from '../users/users.model';
import { UserRole } from './user-role.model';
import { RolesService } from './roles.service';

@Module({
  controllers: [RolesController],
  providers: [RolesService],
  imports: [SequelizeModule.forFeature([Role, User, UserRole])],
  exports: [RolesService],
})
export class RolesModule {}
