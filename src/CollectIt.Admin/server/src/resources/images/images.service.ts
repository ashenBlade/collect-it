import { InjectModel } from '@nestjs/sequelize';
import { NotFoundError } from 'rxjs';
import { ResourcesService } from '../resources.service';
import { Image } from './images.model';

export class ImagesService {
  constructor(
    private readonly resourcesService: ResourcesService,
    @InjectModel(Image) private readonly imageRepository: typeof Image,
  ) {}

  async findImageByIdAsync(id: number): Promise<Image> {
    const image = await this.imageRepository.findByPk(id, {
      include: [{ all: true }],
    });
    if (!image) {
      throw new NotFoundError('Image with specified id not found');
    }
    return image;
  }

  async getAllImagesPagedOrderedAsync(pageNumber: number, pageSize: number) {
    if (pageNumber < 1) throw new RangeError('Page number must be positive');
    if (pageSize < 1) throw new RangeError('Page size must be positive');
    return await this.imageRepository.findAndCountAll({
      include: [{ all: true }],
      limit: pageSize,
      offset: (pageNumber - 1) * pageSize,
      order: [['Id', 'ASC']],
    });
  }

  async deleteImageByIdAsync(id: number) {
    const affected = await this.imageRepository.destroy({
      where: {
        id: id,
      },
      cascade: true,
    });
    if (affected === 0) {
      throw new NotFoundError('Image with specified id not found');
    }
  }

  async changeImageNameAsync(imageId: number, name: string) {
    if (name?.length < 6) {
      throw new Error('Image name length must be greater than 5');
    }

    try {
      await this.resourcesService.changeResourceNameAsync(imageId, name);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundError('No image with provided id found');
      }
      throw e;
    }
  }

  async changeImageTagsAsync(imageId: number, tags: string[]) {
    if (!tags) {
      throw new Error('Tags must be provided');
    }

    try {
      await this.resourcesService.changeResourceTagsAsync(imageId, tags);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundError('No image with specified id found');
      }
      throw e;
    }
  }
}
