import {
    BadRequestException,
    Body,
    Controller,
    Get,
    NotFoundException,
    Param,
    ParseIntPipe,
    Post,
    Query, UsePipes, ValidationPipe
} from '@nestjs/common';
import {SubscriptionsService} from "./subscriptions.service";
import {Authorize} from "../auth/jwt-auth.guard";
import {AuthorizeAdmin} from "../auth/admin-jwt-auth.guard";
import {CreateSubscriptionDto} from "./dto/create-subscription.dto";
import CreationException from "../common/creation.exception";
import {ParseResourceTypePipe} from "../common/parse-resource-type.pipe";
import {ResourceType} from "../common/resource-type";
import {ToReadSubscriptionDto} from "./dto/read-subscription.dto";

@Authorize()
@Controller('api/v1/subscriptions')
export class SubscriptionsController {
    constructor(private subscriptionsService: SubscriptionsService) { }

    @Get(':subscriptionId')
    @UsePipes(ValidationPipe)
    async getSubscriptionById(@Param('subscriptionId', new ParseIntPipe()) subscriptionId: number) {
        const subscription = await this.subscriptionsService.getSubscriptionByIdAsync(subscriptionId);
        if (!subscription) {
            throw new NotFoundException({
                message: `Subscription with id = ${subscriptionId} not found`
            })
        }
        return ToReadSubscriptionDto(subscription);
    }

    @AuthorizeAdmin()
    @Post('')
    async createSubscription(@Body() dto: CreateSubscriptionDto) {
        try {
            const subscription = await this.subscriptionsService.createSubscriptionAsync(
                dto.name,
                dto.description,
                dto.price,
                dto.monthDuration,
                dto.appliedResourceType,
                dto.maxResourcesCount,
                dto.active,
                dto.restriction?.restrictionType,
                dto.restriction?.authorId,
                dto.restriction?.sizeBytes,
                dto.restriction?.daysTo,
                dto.restriction?.daysAfter,
                dto.restriction?.tags
            );
            console.log(`Created subscription: ${JSON.stringify(subscription, null, 2)}`);
            return subscription;
        } catch (e) {
            if (e instanceof CreationException) {
                throw new BadRequestException({
                    message: e.message,
                    errors: e.errors
                })
            }
            throw new BadRequestException({
                message: 'Could not create subscription. Try later.',
                errors: []
            }, 'Something went wrong on server')
        }
    }

    @Get('')
    async getSubscriptionsList(@Query('page_number', new ParseIntPipe())pageNumber: number,
                               @Query('page_size', new ParseIntPipe())pageSize: number,
                               @Query('type', new ParseResourceTypePipe())resourceType: ResourceType) {
        try {
            const paged = await this.subscriptionsService.getSubscriptionsByResourceType(resourceType, pageNumber, pageSize);
            return {
                totalCount: paged.count,
                subscriptions: paged.rows.map(s => ToReadSubscriptionDto(s))
            }
        } catch (e) {
            console.error(e);
            throw new BadRequestException({
                message: 'Something went wrong on server. Try later.'
            })
        }
    }
}
