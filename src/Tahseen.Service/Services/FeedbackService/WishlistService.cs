﻿using AutoMapper;
using Tahseen.Data.IRepositories;
using Tahseen.Data.Repositories;
using Tahseen.Domain.Entities;
using Tahseen.Domain.Entities.Books;
using Tahseen.Domain.Entities.Feedback;
using Tahseen.Domain.Entities.Feedbacks;
using Tahseen.Service.DTOs.Feedbacks.Wishlists;
using Tahseen.Service.Exceptions;
using Tahseen.Service.Interfaces.IFeedbackService;
using Tahseen.Service.Interfaces.IUsersService;

namespace Tahseen.Service.Services.FeedbackService;

public class WishlistService : IWishlistService
{
    private readonly IMapper mapper;
    private readonly IRepository<WishList> repository;
    private readonly IRepository<UserCart> userCartRepository;
    private readonly IRepository<Book> bookRepository;
    public WishlistService(IMapper mapper, IRepository<WishList> repository,
        IRepository<UserCart> userCartRepository,
        IRepository<Book> bookRepository)
    {
        this.mapper = mapper;
        this.repository = repository;
        this.userCartRepository = userCartRepository;
        this.bookRepository = bookRepository;
    }

    public async Task<WishlistForResultDto> AddAsync(WishlistForCreationDto dto)
    {
        var book = await this.bookRepository.SelectByIdAsync(dto.BookId);
        if (book == null || book.IsDeleted)
            throw new TahseenException(404, "Book not found");
        //shu joyini korib chiqamiz
        var cart = await this.userCartRepository.SelectByIdAsync(dto.UserId);

        var wishlist = new WishList
        {
            Id = cart.Id,
            BookId = dto.BookId,
            Status = dto.Status
        };
        var insertedWishlist = await repository.CreateAsync(wishlist);

        return mapper.Map<WishlistForResultDto>(insertedWishlist);
    }

    public async Task<WishlistForResultDto> ModifyAsync(WishlistForUpdateDto dto)
    {
        var wishlist = await this.repository.SelectByIdAsync(dto.Id);
        if (wishlist == null || wishlist.IsDeleted)
            throw new TahseenException(404, "wishlist not found");

        wishlist.BookId = dto.BookId;
        wishlist.Status = dto.Status;
        wishlist.UpdatedAt = DateTime.UtcNow;

        return mapper.Map<WishlistForResultDto>(await this.repository.UpdateAsync(wishlist));
    }

    public async Task<bool> RemoveAsync(long id)
    {
        var wishlist = await this.repository.SelectByIdAsync(id);
        if (wishlist == null || wishlist.IsDeleted)
            throw new TahseenException(404, "Wishlist not found");

        return await this.repository.DeleteAsync(wishlist.Id);
    }

    public IQueryable<WishlistForResultDto> RetrieveAll()
    {
        var wishlists = this.repository.SelectAll().Where(w => !w.IsDeleted);
        return this.mapper.Map<IQueryable<WishlistForResultDto>>(wishlists);
    }

    public async ValueTask<WishlistForResultDto> RetrieveById(long id)
    {
        var wishlist = await this.repository.SelectByIdAsync(id);
        if (wishlist is null || wishlist.IsDeleted)
            throw new TahseenException(404, "Wishlist not found");

        return mapper.Map<WishlistForResultDto>(wishlist);
    }
}