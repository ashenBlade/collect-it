import { InjectModel } from '@nestjs/sequelize';
import { Resource } from './resources.model';
import { NotFoundError } from 'rxjs';

export class ResourcesService {
  constructor(
    @InjectModel(Resource)
    private readonly resourcesRepository: typeof Resource,
  ) {}
  async getResourceByIdAsync(id: number): Promise<Resource> {
    const resource = await this.resourcesRepository.findByPk(id);
    if (!resource) {
      throw new Error('Resource with specified id not found');
    }
    return resource;
  }

  async createResourceAsync(
    name: string,
    ownerId: number,
    tags: string[],
    extension: string,
    uploadDate: Date,
  ): Promise<Resource> {
    if (!name) {
      throw new Error('Resource name not provided');
    }
    if (name.length < 6) {
      throw new Error('Length of resource name must be greater than 5');
    }
    if (!extension) {
      throw new Error('Resource extension not provided');
    }
    if (!tags) {
      tags = [];
    }
    return await this.resourcesRepository.create({
      name: name,
      ownerId: ownerId,
      tags: tags,
      uploadDate: uploadDate,
      extension: extension,
    });
  }

  async deleteResourceByIdAsync(id: number) {
    const affected = await this.resourcesRepository.destroy({
      where: {
        id: id,
      },
      cascade: true,
    });
    if (affected === 0) {
      throw new NotFoundError('Resource with specified id not found');
    }
  }

  async changeResourceNameAsync(id: number, name: string) {
    console.log('In resources service', id, name);
    const affected = await this.resourcesRepository.update(
      {
        name: name,
      },
      {
        where: {
          id: id,
        },
      },
    );
    if (affected[0] === 0) {
      throw new NotFoundError('No resource with provided id found');
    }
  }

  async changeResourceTagsAsync(musicId: number, tags: string[]) {
    if (!tags) {
      throw new Error('No tags provided');
    }

    const affected = await this.resourcesRepository.update(
      {
        tags: tags,
      },
      {
        where: {
          id: musicId,
        },
      },
    );
    if (affected[0] === 0) {
      throw new NotFoundError('No resource with specified id found');
    }
  }
}
