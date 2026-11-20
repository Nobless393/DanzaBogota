using DanzaBogota.Domain.Entity.Entities;
using DanzaBogota.Domain.Entity.Enums;
using DanzaBogota.Domain.Entity.Identities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Infrastructure.Data.Seed
{
    public static class TestDataSeeder
    {
        private const string GestorEmail = "gestor@danzabogota.com";
        private const string GestorPassword = "Gestor!123456";

        private const string GrupoEmail = "grupo@danzabogota.com";
        private const string GrupoPassword = "Grupo!123456";

        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            await IdentitySeeder.SeedRolesAsync(roleManager);

            var gestor = await EnsureUserAsync(userManager, GestorEmail, GestorPassword,
                "Mariana", "Rodríguez", "3221112233", "Gestor");

            var grupoUser = await EnsureUserAsync(userManager, GrupoEmail, GrupoPassword,
                "Carlos", "Méndez", "3005558899", "Grupo");

            if (gestor == null || grupoUser == null) return;

            var localities = await context.Localities.ToDictionaryAsync(l => l.Name);
            var categories = await context.Categories.ToDictionaryAsync(c => c.Name);
            var eventTypes = await context.EventTypes.ToDictionaryAsync(et => et.Name);

            // Grupos con participantes
            var companyRaices = await EnsureGroupAsync(context, "Compañía Raíces",
                "Grupo infantil de danza folclórica del colegio.", "Laura Gómez",
                localities["Chapinero"], categories["Infantil"], grupoUser.Id, useDirectorRut: true);

            var grupoAndino = await EnsureGroupAsync(context, "Grupo Folclórico Andino",
                "Conjunto juvenil de música y danza andina.", "Carlos Méndez",
                localities["Suba"], categories["Juvenil"], grupoUser.Id, useDirectorRut: false);

            var raicesParticipants = await EnsureParticipantsAsync(context, companyRaices.Id, new[]
            {
                ("Sofía", "Martínez", DocumentType.TI, "1012345601", "2017-04-12"),
                ("Mateo", "García", DocumentType.TI, "1012345602", "2016-09-23"),
                ("Valentina", "López", DocumentType.TI, "1012345603", "2018-01-30"),
                ("Samuel", "Torres", DocumentType.TI, "1012345604", "2015-11-07")
            });

            var andinoParticipants = await EnsureParticipantsAsync(context, grupoAndino.Id, new[]
            {
                ("Isabella", "Pérez", DocumentType.CC, "1023456781", "2010-03-15"),
                ("Nicolás", "Ramírez", DocumentType.CC, "1023456782", "2011-07-22"),
                ("Camila", "Sánchez", DocumentType.CC, "1023456783", "2009-12-05"),
                ("Juan", "Gutiérrez", DocumentType.CC, "1023456784", "2012-02-18"),
                ("María", "Fernández", DocumentType.CC, "1023456785", "2010-08-09")
            });

            // Eventos creados por la gestora
            var festival = await EnsureEventAsync(context,
                "Festival de Danza Folclórica 2026",
                "Festival anual de danza folclórica con grupos de toda Bogotá.",
                localities["Teusaquillo"], eventTypes, categories,
                new[] { "Festival", "Encuentro cultural" },
                new[] { "Infantil", "Juvenil", "Adulto", "Mixta" },
                DateTime.Today.AddDays(45), DateTime.Today.AddDays(48),
                "Complejo Cultural Teusaquillo", "Cra 15 # 45-20",
                100, 10, EventStatus.Published, gestor.Id, requiresRut: true);

            var muestra = await EnsureEventAsync(context,
                "Muestra de Danza Urbana",
                "Muestra de danza urbana y breakdance para jóvenes.",
                localities["Kennedy"], eventTypes, categories,
                new[] { "Muestra artística", "Presentación" },
                new[] { "Juvenil", "Adulto" },
                DateTime.Today.AddDays(20), DateTime.Today.AddDays(20),
                "Parque Metropolitano Kennedy", "Av 1ro de Mayo # 65-30",
                60, 8, EventStatus.Published, gestor.Id, requiresRut: false);

            var concurso = await EnsureEventAsync(context,
                "Concurso Nacional de Danza 2025",
                "Concurso nacional ya finalizado de danza en pareja.",
                localities["Usaquén"], eventTypes, categories,
                new[] { "Concurso" },
                new[] { "Juvenil", "Adulto" },
                DateTime.Today.AddDays(-60), DateTime.Today.AddDays(-57),
                "Auditorio Usaquén", "Calle 116 # 9-20",
                80, 6, EventStatus.Closed, gestor.Id, requiresRut: false);

            var taller = await EnsureEventAsync(context,
                "Taller de Danza Contemporánea",
                "Taller práctico de técnica contemporánea, borrador en preparación.",
                localities["Chapinero"], eventTypes, categories,
                new[] { "Taller", "Clase magistral" },
                new[] { "Adulto", "Mixta" },
                DateTime.Today.AddDays(75), DateTime.Today.AddDays(79),
                "Centro Cultural Chapinero", "Cra 7 # 55-10",
                40, 12, EventStatus.Draft, gestor.Id, requiresRut: false);

            // Inscripciones
            await EnsureInscriptionAsync(context, companyRaices.Id, festival.Id,
                ApplicationStatus.Pending, raicesParticipants);

            await EnsureInscriptionAsync(context, grupoAndino.Id, festival.Id,
                ApplicationStatus.Approved, andinoParticipants);
        }

        private static async Task<User?> EnsureUserAsync(
            UserManager<User> userManager,
            string email,
            string password,
            string firstName,
            string lastName,
            string phone,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    DocumentType = "CC",
                    DocumentNumber = email == GestorEmail ? "801234560" : "801234561",
                    PhoneNumber = phone,
                    IsActive = true,
                    EmailConfirmed = true,
                    Created = DateTimeOffset.UtcNow,
                    CreatedBy = "TestDataSeeder",
                    LastModified = DateTimeOffset.UtcNow,
                    LastModifiedBy = "TestDataSeeder"
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded) return null;

                await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                var roles = await userManager.GetRolesAsync(user);
                if (!roles.Contains(role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }

            return user;
        }

        private static async Task<Group?> EnsureGroupAsync(
            ApplicationDbContext context,
            string name,
            string description,
            string directorName,
            Locality locality,
            Category category,
            int userId,
            bool useDirectorRut)
        {
            var group = await context.Groups.FirstOrDefaultAsync(g => g.Name == name);

            if (group != null) return group;

            group = new Group
            {
                Name = name,
                Description = description,
                DirectorName = directorName,
                Phone = "6015551234",
                Email = name.ToLower().Replace(" ", "") + "@grupos.com",
                SocialMediaUrl = null,
                UseDirectorRut = useDirectorRut,
                IsActive = true,
                LocalityId = locality.Id,
                CategoryId = category.Id,
                UserId = userId,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = "TestDataSeeder"
            };

            context.Groups.Add(group);
            await context.SaveChangesAsync();

            return group;
        }

        private static async Task<List<Participant>> EnsureParticipantsAsync(
            ApplicationDbContext context,
            int groupId,
            (string First, string Last, DocumentType DocType, string DocNumber, string BirthDate)[] data)
        {
            var created = new List<Participant>();

            foreach (var (first, last, docType, docNumber, birthDate) in data)
            {
                var birth = DateOnly.Parse(birthDate);
                var existing = await context.Participants
                    .FirstOrDefaultAsync(p => p.GroupId == groupId && p.DocumentNumber == docNumber);

                if (existing != null)
                {
                    created.Add(existing);
                    continue;
                }

                var today = DateOnly.FromDateTime(DateTime.Today);
                var age = today.Year - birth.Year;
                if (birth > today.AddYears(-age)) age--;

                var participant = new Participant
                {
                    FirstName = first,
                    LastName = last,
                    DocumentType = docType,
                    DocumentNumber = docNumber,
                    BirthDate = birth,
                    Age = age,
                    GroupId = groupId,
                    IsActive = true,
                    Created = DateTimeOffset.UtcNow,
                    CreatedBy = "TestDataSeeder"
                };

                context.Participants.Add(participant);
                created.Add(participant);
            }

            await context.SaveChangesAsync();
            return created;
        }

        private static async Task<Event?> EnsureEventAsync(
            ApplicationDbContext context,
            string title,
            string description,
            Locality locality,
            Dictionary<string, Domain.Entity.Entities.EventType> eventTypes,
            Dictionary<string, Category> categories,
            string[] typeNames,
            string[] categoryNames,
            DateTime startDate,
            DateTime endDate,
            string locationName,
            string address,
            int maxParticipants,
            int maxParticipantsPerGroup,
            EventStatus status,
            int createdByUserId,
            bool requiresRut)
        {
            var evt = await context.Events
                .Include(e => e.EventTypes)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.Title == title);

            if (evt != null) return evt;

            var selectedTypes = typeNames
                .Where(eventTypes.ContainsKey)
                .Select(t => eventTypes[t])
                .ToList();

            var selectedCategories = categoryNames
                .Where(categories.ContainsKey)
                .Select(c => categories[c])
                .ToList();

            evt = new Event
            {
                Title = title,
                Description = description,
                ImageUrl = null,
                StartDate = startDate,
                EndDate = endDate,
                LocationName = locationName,
                Address = address,
                LocalityId = locality.Id,
                MaxParticipants = maxParticipants,
                MaxParticipantsPerGroup = maxParticipantsPerGroup,
                Status = status,
                RequiresRut = requiresRut,
                IsActive = true,
                CreatedByUserId = createdByUserId,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = "TestDataSeeder",
                EventTypes = selectedTypes,
                Categories = selectedCategories
            };

            context.Events.Add(evt);
            await context.SaveChangesAsync();

            return evt;
        }

        private static async Task EnsureInscriptionAsync(
            ApplicationDbContext context,
            int groupId,
            int eventId,
            ApplicationStatus status,
            List<Participant> participants)
        {
            var existing = await context.EventApplications
                .FirstOrDefaultAsync(a => a.GroupId == groupId && a.EventId == eventId);

            if (existing != null) return;

            var inscription = new EventApplication
            {
                GroupId = groupId,
                EventId = eventId,
                AppliedAt = DateTimeOffset.UtcNow,
                Status = status,
                Observations = status == ApplicationStatus.Approved ? "Aprobada por la gestora del evento." : null,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = "TestDataSeeder"
            };

            context.EventApplications.Add(inscription);
            await context.SaveChangesAsync();

            foreach (var participant in participants)
            {
                context.ApplicationParticipants.Add(new ApplicationParticipant
                {
                    ApplicationId = inscription.Id,
                    ParticipantId = participant.Id,
                    Created = DateTimeOffset.UtcNow
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
