import Resource from "./resource";

export default class Image extends Resource {
    constructor(id: number, name: string, uploadDate: Date, filename: string, tags: string[], extension: string, ownerId: number) {
        super(id, name, uploadDate, filename, tags, extension, ownerId);
    }
}