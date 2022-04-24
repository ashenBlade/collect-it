import {
    BadRequestException,
    Controller, Delete,
    Get,
    NotFoundException,
    Param,
    ParseIntPipe,
    Query
} from "@nestjs/common";
import {MusicsService} from "./musics.service";
import {Authorize} from "../../auth/jwt-auth.guard";
import {ReadMusicDto, ToReadMusicDto} from "./dtos/read-music.dto";
import {NotFoundError} from "rxjs";
import {AuthorizeAdmin} from "../../auth/admin-jwt-auth.guard";

@Authorize()
@Controller('api/v1/musics')
export class MusicsController {
    constructor(private readonly musicsService: MusicsService) {  }

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
        const musics = await this.musicsService.getAllMusicsPagedOrderedAsync(pageNumber, pageSize);
        const dtos: ReadMusicDto[] = musics.rows.map(ToReadMusicDto);
        return {
            totalCount: musics.count,
            musics: dtos
        }
    }

    @Get(':musicId')
    async findMusicById(@Param('musicId', new ParseIntPipe()) musicId: number): Promise<ReadMusicDto> {
        try {
            const music = await this.musicsService.findMusicByIdAsync(musicId);
            return ToReadMusicDto(music);
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
            await this.musicsService.deleteMusicByIdAsync(musicId);
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

