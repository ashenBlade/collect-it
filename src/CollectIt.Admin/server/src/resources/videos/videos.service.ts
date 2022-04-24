import {ResourcesService} from "../resources.service";
import {InjectModel} from "@nestjs/sequelize";
import {Music} from "../musics/musics.model";
import {Resource} from "../resources.model";
import {NotFoundError} from "rxjs";
import {Video} from "./videos.model";

export class VideosService {
    constructor(private readonly resourcesService: ResourcesService,
                @InjectModel(Video) private readonly videosRepository: typeof Video) {  }
    async createVideoAsync(name: string, ownerId: number, tags: string[], extension: string, uploadDate: Date, duration: number): Promise<Music> {
        if (!name) {
            throw new Error('Video name not provided');
        }
        if (!tags) {
            tags = []
        }
        if (!extension) {
            throw new Error('Video extension is not provided');
        }
        if (duration < 1) {
            throw new RangeError('Video duration must be positive');
        }
        let resource: Resource | null;
        let music: Music | null;
        try {
            resource = await this.resourcesService.createResourceAsync(name, ownerId, tags, extension, uploadDate);
            const music = await this.videosRepository.create({
                id: resource.id,
                duration: duration
            });
            if (!music) {
                throw new Error('Could not create video object');
            }
            return music;
        } catch (e) {
            if (resource) {
                await this.resourcesService.deleteResourceByIdAsync(resource.id);
            }
            if (music) {
                await music.destroy();
                await music.save();
            }
            throw e;
        }
    }

    async findVideoByIdAsync(id: number): Promise<Music> {
        const music = await this.videosRepository.findByPk(id, {
            include: [{all: true}]
        });
        if (!music) {
            throw new NotFoundError('Video with specified id not found');
        }
        return music;
    }

    async getAllVideosPagedOrderedAsync(pageNumber: number, pageSize: number) {
        if (pageNumber < 1) throw new RangeError('Page number must be positive');
        if (pageSize < 1) throw new RangeError('Page size must be positive');
        return await this.videosRepository.findAndCountAll({
            include: [{all: true,}],
            limit: pageSize,
            offset: (pageNumber - 1) * pageSize,
            order: [['Id', 'ASC']]
        });
    }

    async deleteVideoByIdAsync(id: number) {
        const affected = await this.videosRepository.destroy({
            where: {
                id: id
            },
            cascade: true
        });
        if (affected === 0) {
            throw new NotFoundError('Video with specified id not found');
        }
    }
}