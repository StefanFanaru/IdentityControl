using System;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;

namespace IdentityControl.API.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }

        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public DateTime InsertDate { get; set; }
        public DateTime? DisableDate { get; set; }
        public Address Address { get; private set; }
        public bool AcceptsInformativeEmails { get; private set; }
        public bool HasPicture { get; private set; }
        public DateTime? LastModified { get; private set; }
        public DateTime? LastLogin { get; set; }
        public string PictureUrl { get; private set; }

        public string BlogId { get; set; }

        public static ApplicationUser Create(string email, string lastName, string firstName, bool acceptsEmails)
        {
            Guard.Against.NullOrEmpty(email, nameof(email));
            Guard.Against.NullOrEmpty(lastName, nameof(lastName));
            Guard.Against.NullOrEmpty(firstName, nameof(firstName));

            return new ApplicationUser
            {
                UserName = email,
                Email = email,
                LastName = lastName,
                FirstName = firstName,
                AcceptsInformativeEmails = acceptsEmails,
                InsertDate = DateTime.UtcNow
            };
        }


        public void UpdateAddress(Address address)
        {
            Guard.Against.NullOrEmpty(address.City, nameof(address.City));
            Guard.Against.NullOrEmpty(address.Country, nameof(address.Country));
            Guard.Against.NullOrEmpty(address.County, nameof(address.County));

            Address = address;
        }

        public void UpdatePicture(string pictureUrl)
        {
            Guard.Against.NullOrEmpty(pictureUrl, nameof(pictureUrl));

            HasPicture = true;
            PictureUrl = pictureUrl;
            MarkModified();
        }

        public void DeletePicture()
        {
            HasPicture = false;
            PictureUrl = null;
            MarkModified();
        }

        public void Update(string firstName, string lastName, bool acceptsInformativeEmails)
        {
            Guard.Against.NullOrEmpty(firstName, nameof(firstName));
            Guard.Against.NullOrEmpty(lastName, nameof(lastName));

            FirstName = firstName;
            LastName = lastName;
            AcceptsInformativeEmails = acceptsInformativeEmails;
            MarkModified();
        }

        public void UpdateContact(string email, string phoneNumber)
        {
            Guard.Against.NullOrEmpty(email, nameof(email));
            Guard.Against.NullOrEmpty(phoneNumber, nameof(phoneNumber));

            Email = email;
            UserName = email;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = false;
            MarkModified();
        }

        public void MarkModified()
        {
            LastModified = DateTime.UtcNow;
        }
    }
}