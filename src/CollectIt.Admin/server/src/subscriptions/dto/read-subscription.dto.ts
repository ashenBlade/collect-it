import {ReadRestrictionDto} from "../restrictions/dto/read-restriction.dto";

export class ReadSubscriptionDto {
    readonly id: number;
    readonly name: string;
    readonly description: string;
    readonly maxResourcesCount: number;
    readonly price: number;
    readonly monthDuration: number;
    readonly restriction: ReadRestrictionDto | null;
    readonly active: boolean;
}