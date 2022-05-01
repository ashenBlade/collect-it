import { Video } from '../videos.model';

export interface ReadVideoDto {
  readonly id: number;
  readonly name: string;
  readonly duration: number;
  readonly ownerId: number;
  readonly uploadDate: Date;
  readonly tags: string[];
  readonly extension: string;
  readonly filename: string;
}

export const ToReadVideoDto = (v: Video): ReadVideoDto => ({
  id: v.id,
  name: v.resource.name,
  duration: v.duration,
  ownerId: v.resource.ownerId,
  uploadDate: v.resource.uploadDate,
  tags: v.resource.tags,
  extension: v.resource.extension,
  filename: v.resource.filename,
});
