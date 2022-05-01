import {
  BadRequestException,
  Body,
  Controller,
  Delete,
  Get,
  NotFoundException,
  Param,
  ParseIntPipe,
  Post,
  Query,
} from '@nestjs/common';
import { NotFoundError } from 'rxjs';
import { AuthorizeAdmin } from 'src/auth/admin-jwt-auth.guard';
import { Authorize } from 'src/auth/jwt-auth.guard';
import { ReadImageDto, ToReadImageDto } from './dtos/read-image.dto';
import { ImagesService } from './images.service';

@Authorize()
@Controller('api/v1/images')
export class ImagesController {
  constructor(private readonly imageService: ImagesService) {}

  @Get('')
  async getAllImagesPaged(
    @Query('page_number', new ParseIntPipe()) pageNumber: number,
    @Query('page_size', new ParseIntPipe()) pageSize: number,
  ) {
    if (pageNumber < 1) {
      throw new BadRequestException({
        message: 'Page number must be positive',
      });
    }
    if (pageSize < 1) {
      throw new BadRequestException({
        message: 'Page size must be positive',
      });
    }
    const images = await this.imageService.getAllImagesPagedOrderedAsync(
      pageNumber,
      pageSize,
    );
    const dtos: ReadImageDto[] = images.rows.map(ToReadImageDto);
    return {
      totalCount: images.count,
      images: dtos,
    };
  }

  @Get(':imageId')
  async findImageById(
    @Param('imageId', new ParseIntPipe()) imageId: number,
  ): Promise<ReadImageDto> {
    try {
      const image = await this.imageService.findImageByIdAsync(imageId);
      return ToReadImageDto(image);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Image with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: e.message,
      });
    }
  }

  @Delete(':imageId')
  @AuthorizeAdmin()
  async deleteImageById(@Param('imageId', new ParseIntPipe()) imageId: number) {
    try {
      await this.imageService.deleteImageByIdAsync(imageId);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Image with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: 'Unexpected error occurred while deleting image. Try later.',
      });
    }
  }

  @Post(':imageId/name')
  @AuthorizeAdmin()
  async changeImageName(
    @Param('imageId', new ParseIntPipe()) imageId: number,
    @Body('name') name: string,
  ) {
    try {
      if (name?.length < 6) {
        throw new Error('Length of name must be greater than 6');
      }
      await this.imageService.changeImageNameAsync(imageId, name);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Image with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: 'Unexpected error occurred while processing request',
      });
    }
  }

  @Post(':imageId/tags')
  @AuthorizeAdmin()
  async changeImageTags(
    @Param('imageId', new ParseIntPipe()) imageId: number,
    @Body('tags') tags: string[],
  ) {
    try {
      if (!tags) {
        throw new Error('No tags provided');
      }
      await this.imageService.changeImageTagsAsync(imageId, tags);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Image with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: 'Unexpected error occurred while processing request',
      });
    }
  }
}
