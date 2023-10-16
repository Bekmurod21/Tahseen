using AutoMapper;
using Tahseen.Data.IRepositories;
using Tahseen.Domain.Entities.Books;
using Tahseen.Domain.Entities.Reservations;
using Tahseen.Service.DTOs.Books.Author;
using Tahseen.Service.DTOs.Reservations;
using Tahseen.Service.DTOs.Reservitions;

namespace Tahseen.Service.Services.Reservations;

public class ReservationService
{
    private readonly IMapper _mapper;
    private readonly IRepository<Reservation> _repository;

    public ReservationService(IMapper mapper, IRepository<Reservation> repository)
    {
        this._mapper = mapper;
        this._repository = repository;
    }

    public async Task<ReservationForResultDto> AddAsync(ReservationForCreationDto dto)
    {
        var reservation = _mapper.Map<Reservation>(dto);
        var result= await _repository.CreateAsync(reservation);
        return _mapper.Map<ReservationForResultDto>(result);
    }

    public async Task<ReservationForResultDto> ModifyAsync(long id, ReservationForUpdateDto dto)
    {
        var reservation = await _repository.SelectByIdAsync(id);
        if (reservation is not null && !reservation.IsDeleted)
        {
            var mappedReservation = _mapper.Map<Reservation>(dto);
            mappedReservation.UpdatedAt = DateTime.UtcNow;
            var result = await _repository.UpdateAsync(mappedReservation);
            return _mapper.Map<ReservationForResultDto>(result);
        }
        throw new Exception("Reservation not found");
    }

    public async Task<bool> RemoveAsync(long id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async ValueTask<ReservationForResultDto> RetrieveByIdAsync(long id)
    {
        var reservation = await _repository.SelectByIdAsync(id);
        if (reservation is not null && !reservation.IsDeleted)
            return _mapper.Map<ReservationForResultDto>(reservation);
        
        throw new Exception("Reservation  not found");
    }
}