import { Image } from '../images.model';

export interface ReadImageDto {
  readonly id: number;
  readonly name: string;
  readonly ownerId: number;
  readonly uploadDate: Date;
  readonly tags: string[];
  readonly extension: string;
  readonly filename: string;
}

export const ToReadImageDto = (i: Image): ReadImageDto => ({
  id: i.id,
  name: i.resource.name,
  ownerId: i.resource.ownerId,
  tags: i.resource.tags,
  extension: i.resource.extension,
  uploadDate: i.resource.uploadDate,
  filename: i.resource.filename,
});
