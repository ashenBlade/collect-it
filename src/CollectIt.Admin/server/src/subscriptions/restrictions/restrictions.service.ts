import { Injectable } from '@nestjs/common';
import { InjectModel } from '@nestjs/sequelize';
import { Restriction } from './restriction.model';
import { RestrictionType } from './restriction-type';

@Injectable()
export class RestrictionsService {
  constructor(
    @InjectModel(Restriction)
    private restrictionsRepository: typeof Restriction,
  ) {}

  async createAuthorRestrictionAsync(authorId: number) {
    const restriction = await this.restrictionsRepository.create({
      restrictionType: RestrictionType.Author,
      authorId: authorId,
      daysAfter: null,
      daysTo: null,
      tags: null,
      sizeBytes: null,
    });
    return restriction;
  }

  async createDaysToRestrictionAsync(daysTo: number): Promise<Restriction> {
    const restriction = await this.restrictionsRepository.create({
      restrictionType: RestrictionType.DaysTo,
      daysTo: daysTo,
      authorId: null,
      daysAfter: null,
      tags: null,
      sizeBytes: null,
    });
    return restriction;
  }

  async createDaysAfterRestrictionAsync(
    daysAfter: number,
  ): Promise<Restriction> {
    const restriction = await this.restrictionsRepository.create({
      restrictionType: RestrictionType.DaysAfter,
      daysAfter: daysAfter,
      authorId: null,
      daysTo: null,
      tags: null,
      sizeBytes: null,
    });
    return restriction;
  }

  async createTagsRestrictionAsync(tags: string[]): Promise<Restriction> {
    tags = tags.filter(t => t !== '');
    const restriction = await this.restrictionsRepository.create({
      restrictionType: RestrictionType.Tags,
      tags: tags,
      daysTo: null,
      authorId: null,
      daysAfter: null,
      sizeBytes: null,
    });
    return restriction;
  }

  async createSizeRestrictionAsync(size: number): Promise<Restriction> {
    const restriction = await this.restrictionsRepository.create({
      restrictionType: RestrictionType.Size,
      sizeBytes: size,
      daysTo: null,
      authorId: null,
      daysAfter: null,
      tags: null,
    });
    return restriction;
  }

  async findRestrictionById(id: number): Promise<Restriction> {
    const restriction = await this.restrictionsRepository.findByPk(id);
    return restriction;
  }

  async deleteRestrictionById(id: number) {
    await this.restrictionsRepository.destroy({
      where: {
        id: id,
      },
    });
  }
}
