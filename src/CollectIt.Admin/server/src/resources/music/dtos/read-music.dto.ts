import {Music} from "../musics.model";

export interface ReadMusicDto {
    readonly id: number;
    readonly name: string;
    readonly duration: number;
    readonly ownerId: number;
    readonly uploadDate: Date;
    readonly tags: string[];
    readonly extension: string;
}

export const ToReadMusicDto = (m: Music): ReadMusicDto => ({
    id: m.id,
    duration: m.duration,
    name: m.resource.name,
    ownerId: m.resource.ownerId,
    tags: m.resource.tags,
    extension: m.resource.extension,
    uploadDate: m.resource.uploadDate
    // id: 0,
    // duration: 0,
    // name: '',
    // uploadDate: new Date(),
    // extension: '',
    // tags: [],
    // ownerId: 1
});