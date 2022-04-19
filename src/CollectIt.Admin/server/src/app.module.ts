import { Module } from '@nestjs/common';
import { SequelizeModule } from '@nestjs/sequelize'
import { UsersModule } from './users/users.module';
import {ConfigModule} from "@nestjs/config";
import {User} from "./users/users.model";
import { RolesService } from './roles/roles.service';
import { RolesModule } from './roles/roles.module';
import {Role} from "./roles/roles.model";
import {UserRole} from "./roles/user-role.model";
import { AuthModule } from './auth/auth.module';
import { SubscriptionsService } from './subscriptions/subscriptions.service';
import { SubscriptionsController } from './subscriptions/subscriptions.controller';
import { SubscriptionsModule } from './subscriptions/subscriptions.module';
import { RestrictionsService } from './subscriptions/restrictions/restrictions.service';
import {Restriction} from "./subscriptions/restrictions/restriction.model";
import {Subscription} from "./subscriptions/subscriptions.model";

@Module({
  controllers: [],
  providers: [],
  imports: [
      ConfigModule.forRoot({
          envFilePath: `.${process.env.NODE_ENV}.env`
      }),
      SequelizeModule.forRoot({
          dialect: 'postgres',
          host: process.env.POSTGRES_HOST,
          port: Number(process.env.POSTGRES_PORT),
          username: process.env.POSTGRES_USER,
          password: String(process.env.POSTGRES_PASSWORD),
          database: process.env.POSTGRES_DB,
          models: [User, Role, UserRole, Restriction, Subscription],
      }),
      UsersModule,
      RolesModule,
      AuthModule,
      SubscriptionsModule
  ],
})
export class AppModule {

}
