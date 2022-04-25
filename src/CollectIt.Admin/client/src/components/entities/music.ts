import Resource from "./resource";

export default class Music extends Resource {
    duration: number;
    constructor(id: number, name: string, uploadDate: Date, filename: string, tags: string[], extension: string, ownerId: number, duration: number) {
        super(id, name, uploadDate, filename, tags, extension, ownerId);
        this.duration = duration;
    }
}