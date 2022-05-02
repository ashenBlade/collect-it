import { Module } from '@nestjs/common';
import { SubscriptionsService } from './subscriptions.service';
import { RestrictionsService } from './restrictions/restrictions.service';
import { SubscriptionsController } from './subscriptions.controller';
import { SequelizeModule } from '@nestjs/sequelize';
import { Restriction } from './restrictions/restriction.model';
import { Subscription } from './subscriptions.model';
import { AuthModule } from '../auth/auth.module';

@Module({
  controllers: [SubscriptionsController],
  providers: [SubscriptionsService, RestrictionsService],
  imports: [
    SequelizeModule.forFeature([Restriction, Subscription]),
    AuthModule,
  ],
})
export class SubscriptionsModule {}
