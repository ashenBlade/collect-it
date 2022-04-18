import { Module } from '@nestjs/common';
import { SequelizeModule } from '@nestjs/sequelize'
import { UsersModule } from './users/users.module';
import {UsersService} from "./users/users.service";
import {UsersController} from "./users/users.controller";
import {ConfigModule} from "@nestjs/config";

@Module({
  controllers: [UsersController],
  providers: [UsersService],
  imports: [
      ConfigModule.forRoot({
          envFilePath: `.${process.env.NODE_ENV}.env`
      }),
      SequelizeModule.forRoot({
          dialect: 'postgres',
          host: process.env.POSGRES_HOST,
          port: Number(process.env.POSGRES_PORT),
          username: process.env.POSTGRES_USER,
          password: String(process.env.POSGRES_PASSWORD),
          database: process.env.POSTGRES_DB,
          models: []
      }),
      UsersModule
  ],
})
export class AppModule {

}
