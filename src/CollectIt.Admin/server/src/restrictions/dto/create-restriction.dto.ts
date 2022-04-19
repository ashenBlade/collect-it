import {RestrictionType} from "../restriction-type";
import {IsInt, Min} from "class-validator";

export class CreateRestrictionDto {
    readonly restrictionType: RestrictionType;
    @IsInt()
    readonly authorId: number | null;
    @IsInt()
    @Min(1)
    readonly daysAfter: number | null;
    @Min(1)
    readonly daysTo: number | null;
    @Min(1)
    readonly sizeBytes: number | null;
    readonly tags: string[] | null;
}