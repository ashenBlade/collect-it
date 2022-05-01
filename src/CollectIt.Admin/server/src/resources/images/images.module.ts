import { forwardRef, Module } from '@nestjs/common';
import { SequelizeModule } from '@nestjs/sequelize';
import { AuthModule } from 'src/auth/auth.module';
import { Resource } from '../resources.model';
import { ResourcesModule } from '../resources.module';
import { ImagesController } from './images.controller';
import { ImagesService } from './images.service';
import { Image } from './images.model';

@Module({
  providers: [ImagesService],
  controllers: [ImagesController],
  imports: [
    SequelizeModule.forFeature([Image, Resource]),
    forwardRef(() => ResourcesModule),
    AuthModule,
  ],
})
export class ImagesModule {}
