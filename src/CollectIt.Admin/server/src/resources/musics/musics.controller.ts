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
  UsePipes,
  ValidationPipe,
} from '@nestjs/common';
import { MusicsService } from './musics.service';
import { Authorize } from '../../auth/jwt-auth.guard';
import { ReadMusicDto, ToReadMusicDto } from './dtos/read-music.dto';
import { NotFoundError } from 'rxjs';
import { AuthorizeAdmin } from '../../auth/admin-jwt-auth.guard';
import { ChangeTagsDto } from '../common/change-tags.dto';
import { ChangeNameDto } from '../common/change-name.dto';

@Authorize()
@Controller('api/v1/musics')
export class MusicsController {
  constructor(private readonly musicsService: MusicsService) {}

  @Get('')
  async getAllMusicsPaged(
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
    const musics = await this.musicsService.getAllMusicsPagedOrderedAsync(
      pageNumber,
      pageSize,
    );
    const dtos: ReadMusicDto[] = musics.rows.map(ToReadMusicDto);
    return {
      totalCount: musics.count,
      musics: dtos,
    };
  }

  @Get(':musicId')
  async findMusicById(
    @Param('musicId', new ParseIntPipe()) musicId: number,
  ): Promise<ReadMusicDto> {
    try {
      const music = await this.musicsService.findMusicByIdAsync(musicId);
      return ToReadMusicDto(music);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Music with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: e.message,
      });
    }
  }

  @Delete(':musicId')
  @AuthorizeAdmin()
  async deleteMusicById(@Param('musicId', new ParseIntPipe()) musicId: number) {
    try {
      await this.musicsService.deleteMusicByIdAsync(musicId);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Music with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: 'Unexpected error occurred while deleting musics. Try later.',
      });
    }
  }

  @Post(':musicId/name')
  @AuthorizeAdmin()
  async changeMusicName(
    @Param('musicId', new ParseIntPipe()) musicId: number,
    @Body() { name }: ChangeNameDto,
  ) {
    try {
      if (!name) {
        throw new Error('No name provided');
      }
      if (name.length < 6) {
        throw new Error('Length of name must be greater than 6');
      }
      await this.musicsService.changeMusicNameAsync(musicId, name);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Music with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: 'Unexpected error occurred while processing request',
      });
    }
  }

  @Post(':musicId/tags')
  @AuthorizeAdmin()
  @UsePipes(ValidationPipe)
  async changeMusicTags(
    @Param('musicId', new ParseIntPipe()) musicId: number,
    @Body() { tags }: ChangeTagsDto,
  ) {
    try {
      if (!tags) {
        throw new Error('No tags provided');
      }
      await this.musicsService.changeMusicTagsAsync(musicId, tags);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'Music with specified id not found',
        });
      }
      console.error(e);
      throw new BadRequestException({
        message: 'Unexpected error occurred while processing request',
      });
    }
  }
}
