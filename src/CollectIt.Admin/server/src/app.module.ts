import {Module} from '@nestjs/common';
import {SequelizeModule} from '@nestjs/sequelize';
import {UsersModule} from './users/users.module';
import {ConfigModule} from '@nestjs/config';
import {User} from './users/users.model';
import {RolesModule} from './roles/roles.module';
import {Role} from './roles/roles.model';
import {UserRole} from './roles/user-role.model';
import {AuthModule} from './auth/auth.module';
import {SubscriptionsModule} from './subscriptions/subscriptions.module';
import {Restriction} from './subscriptions/restrictions/restriction.model';
import {Subscription} from './subscriptions/subscriptions.model';
import {ResourcesModule} from './resources/resources.module';
import {Music} from './resources/musics/musics.model';
import {Resource} from './resources/resources.model';
import {Video} from './resources/videos/videos.model';
import {Image} from './resources/images/images.model';

@Module({
  controllers: [],
  providers: [],
  imports: [
    ConfigModule.forRoot({
      envFilePath: `.${process.env.NODE_ENV}.env`,
    }),
    SequelizeModule.forRoot({
        dialect: 'postgres',
        host: process.env.POSTGRES_HOST,
        port: Number(process.env.POSTGRES_PORT),
        username: process.env.POSTGRES_USER,
        password: String(process.env.POSTGRES_PASSWORD),
        database: process.env.POSTGRES_DB,
        // ssl: true,
        dialectOptions: {
            ssl: {
                require: true,
                rejectUnauthorized: false
            }
        },
        models: [
            User,
            Role,
            UserRole,
            Restriction,
            Subscription,
            Resource,
            Music,
            Video,
            Image,
      ],
    }),
    UsersModule,
    RolesModule,
    AuthModule,
    SubscriptionsModule,
    ResourcesModule,
  ],
})
export class AppModule {}
