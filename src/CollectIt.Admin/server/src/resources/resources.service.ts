import {InjectModel} from "@nestjs/sequelize";
import {Resource} from "./resources.model";

export class ResourcesService {
    constructor(@InjectModel(Resource)private readonly resourcesRepository: typeof Resource) {  }
    async getResourceByIdAsync(id: number): Promise<Resource> {
        const resource = await this.resourcesRepository.findByPk(id);
        if (!resource) {
            throw new Error('Resource with specified id not found')
        }
        return resource;
    }

    async createResourceAsync(name: string, ownerId: number, tags: string[], extension: string, uploadDate: Date): Promise<Resource> {
        if (!name) {
            throw new Error('Resource name not provided');
        }
        if (!extension) {
            throw new Error('Resource extension not provided')
        }
        if (!tags) {
            tags = []
        }
        const resource = await this.resourcesRepository.create({
            name: name,
            ownerId: ownerId,
            tags: tags,
            uploadDate: uploadDate,
            extension: extension
        });
        return resource;
    }

    async deleteResourceByIdAsync(id: number) {
        const affected = await this.resourcesRepository.destroy({
            where: {
                id: id
            }
        });
        if (affected === 0) {
            throw new Error('Resource with specified id not found');
        }
    }
}