import { forwardRef, Module } from '@nestjs/common';
import { SequelizeModule } from '@nestjs/sequelize';
import { Music } from './musics.model';
import { MusicsService } from './musics.service';
import { MusicsController } from './musics.controller';
import { ResourcesModule } from '../resources.module';
import { AuthModule } from '../../auth/auth.module';
import { Resource } from '../resources.model';

@Module({
  providers: [MusicsService],
  controllers: [MusicsController],
  imports: [
    SequelizeModule.forFeature([Music, Resource]),
    forwardRef(() => ResourcesModule),
    AuthModule,
  ],
})
export class MusicsModule {}
