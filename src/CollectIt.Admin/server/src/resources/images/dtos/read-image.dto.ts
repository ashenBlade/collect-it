import {Image} from "../images.model";

export interface ReadImageDto {
    readonly id: number;
    readonly name: string;
    readonly ownerId: number;
    readonly uploadDate: Date;
    readonly tags: string[];
    readonly extension: string;
}

export const ToReadImageDto = (img: Image): ReadImageDto => ({
    id: img.id,
    name: img.resource.name,
    ownerId: img.resource.ownerId,
    tags: img.resource.tags,
    extension: img.resource.extension,
    uploadDate: img.resource.uploadDate
});