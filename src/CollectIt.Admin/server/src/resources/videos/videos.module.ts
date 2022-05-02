import { forwardRef, Module } from '@nestjs/common';
import { VideosService } from './videos.service';
import { SequelizeModule } from '@nestjs/sequelize';
import { Video } from './videos.model';
import { Resource } from '../resources.model';
import { ResourcesModule } from '../resources.module';
import { AuthModule } from '../../auth/auth.module';
import { VideosController } from './videos.controller';

@Module({
  providers: [VideosService],
  controllers: [VideosController],
  imports: [
    SequelizeModule.forFeature([Video, Resource]),
    forwardRef(() => ResourcesModule),
    AuthModule,
  ],
})
export class VideosModule {}
