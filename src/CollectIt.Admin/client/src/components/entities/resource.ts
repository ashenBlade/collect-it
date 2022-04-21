export default abstract class Resource {
    protected constructor(id: number, name: string, uploadDate: Date, filename: string, tags: string[], extension: string, ownerId: number) {
        this.id = id;
        this.ownerId = ownerId;
        this.name = name;
        this.uploadDate = uploadDate;
        this.extension = extension;
        this.tags = tags;
        this.filename = filename;
    }
    id: number;
    readonly name: string;
    readonly uploadDate: Date;
    readonly filename: string;
    readonly tags: string[];
    readonly extension: string;
    readonly ownerId: number;
}