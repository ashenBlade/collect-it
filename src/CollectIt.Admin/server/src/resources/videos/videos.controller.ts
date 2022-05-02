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
import { VideosService } from './videos.service';
import { Authorize } from '../../auth/jwt-auth.guard';
import { ReadVideoDto, ToReadVideoDto } from './dtos/read-video.dto';
import { NotFoundError } from 'rxjs';
import { AuthorizeAdmin } from '../../auth/admin-jwt-auth.guard';

@Authorize()
@Controller('api/v1/videos')
export class VideosController {
  constructor(private readonly videosService: VideosService) {}

  @Get('')
  async getAllVideosPaged(
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
    const videos = await this.videosService.getAllVideosPagedOrderedAsync(
      pageNumber,
      pageSize,
    );
    const dtos: ReadVideoDto[] = videos.rows.map(ToReadVideoDto);
    return {
      totalCount: videos.count,
      videos: dtos,
    };
  }

  @Get(':videoId')
  async findVideoById(
    @Param('videoId', new ParseIntPipe()) videoId: number,
  ): Promise<ReadVideoDto> {
    try {
      const video = await this.videosService.findVideoByIdAsync(videoId);
      return ToReadVideoDto(video);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Video with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: e.message,
      });
    }
  }

  @Delete(':videoId')
  @AuthorizeAdmin()
  async deleteVideoById(@Param('videoId', new ParseIntPipe()) videoId: number) {
    try {
      await this.videosService.deleteVideoByIdAsync(videoId);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Video with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: 'Unexpected error occurred while deleting video. Try later.',
      });
    }
  }

  @Post(':videoId/name')
  @AuthorizeAdmin()
  async changeVideoName(
    @Param('videoId', new ParseIntPipe()) videoId: number,
    @Body('name') name: string,
  ) {
    try {
      if (name?.length < 6) {
        throw new Error('Length of name must be greater than 6');
      }
      await this.videosService.changeVideoNameAsync(videoId, name);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Video with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: 'Unexpected error occurred while processing request',
      });
    }
  }

  @Post(':videoId/tags')
  @AuthorizeAdmin()
  async changeVideoTags(
    @Param('videoId', new ParseIntPipe()) videoId: number,
    @Body('tags') tags: string[],
  ) {
    try {
      if (!tags) {
        throw new Error('No tags provided');
      }
      await this.videosService.changeVideoTagsAsync(videoId, tags);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Video with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: 'Unexpected error occurred while processing request',
      });
    }
  }
}
