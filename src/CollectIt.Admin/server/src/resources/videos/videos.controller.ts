import {
    BadRequestException,
    Controller, Delete,
    Get,
    NotFoundException,
    Param,
    ParseIntPipe,
    Query
} from "@nestjs/common";
import {VideosService} from "./videos.service";
import {Authorize} from "../../auth/jwt-auth.guard";
import {ReadVideoDto, ToReadVideoDto} from "./dtos/read-video.dto";
import {NotFoundError} from "rxjs";
import {AuthorizeAdmin} from "../../auth/admin-jwt-auth.guard";

@Authorize()
@Controller('api/v1/videos')
export class VideosController {
    constructor(private readonly videosService: VideosService) {  }

    @Get('')
    async getAllMusicsPaged(@Query('page_number') pageNumber: number,
                            @Query('page_size') pageSize: number) {
        if (pageNumber < 1) {
            throw new BadRequestException({
                message: 'Page number must be positive'
            });
        }
        if (pageSize < 1) {
            throw new BadRequestException({
                message: 'Page size must be positive'
            });
        }
        const musics = await this.videosService.getAllVideosPagedOrderedAsync(pageNumber, pageSize);
        const dtos: ReadVideoDto[] = musics.rows.map(ToReadVideoDto);
        return {
            totalCount: musics.count,
            musics: dtos
        }
    }

    @Get(':musicId')
    async findMusicById(@Param('musicId', new ParseIntPipe()) musicId: number): Promise<ReadVideoDto> {
        try {
            const music = await this.videosService.findVideoByIdAsync(musicId);
            return ToReadVideoDto(music);
        } catch (e) {
            if (e instanceof NotFoundError) {
                throw new NotFoundException({
                    message: 'Music with specified id not found'
                })
            }
            console.error(e)
            throw new BadRequestException({
                message: e.message
            })
        }
    }

    @Delete(':musicId')
    @AuthorizeAdmin()
    async deleteMusicById(@Param('musicId', new ParseIntPipe()) musicId: number) {
        try {
            await this.videosService.deleteVideoByIdAsync(musicId);
        } catch (e) {
            if (e instanceof NotFoundError) {
                throw new NotFoundException({
                    message: 'Music with specified id not found'
                })
            }
            throw new BadRequestException({
                message: 'Unexpected error occurred while deleting music. Try later.'
            })
        }
    }
}

