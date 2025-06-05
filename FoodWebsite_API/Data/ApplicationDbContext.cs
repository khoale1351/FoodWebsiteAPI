using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FoodWebsite_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<Ingredient> Ingredients { get; set; }

        public virtual DbSet<Province> Provinces { get; set; }

        public virtual DbSet<Rating> Ratings { get; set; }

        public virtual DbSet<Recipe> Recipes { get; set; }

        public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }

        public virtual DbSet<RecipeStep> RecipeSteps { get; set; }

        public virtual DbSet<Specialty> Specialties { get; set; }

        public virtual DbSet<SpecialtyImage> SpecialtyImages { get; set; }

        public virtual DbSet<UserFavoriteRecipe> UserFavoriteRecipes { get; set; }

        public virtual DbSet<UserIngredient> UserIngredients { get; set; }

        public virtual DbSet<UserViewHistory> UserViewHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationships
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.UserName).HasMaxLength(50);
                entity.Property(u => u.City).HasMaxLength(100);
                entity.Property(u => u.Address).HasMaxLength(300);
                entity.Property(u => u.PhoneNumber).HasMaxLength(16).HasColumnType("varchar(16)");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Province__3214EC079C64CD5E");

                entity.HasIndex(e => new { e.Name, e.Version }).IsUnique();

                entity.Property(e => e.Region).HasMaxLength(50).IsRequired();
                entity.Property(e => e.RegionPlain).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.NamePlain).HasMaxLength(100);
                entity.Property(e => e.Description);
                entity.Property(e => e.Version).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();              
                entity.Property(e => e.ImageUrl).HasMaxLength(255);
            });

            modelBuilder.Entity<Specialty>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Specialt__3214EC07234229EE");
               
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.NamePlain).HasMaxLength(100);
                entity.Property(e => e.Description);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
                entity.Property(e => e.UpdatedAt);
                entity.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();

                entity.HasOne(d => d.Province).WithMany(p => p.Specialties)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Specialti__Provi__6477ECF3");
            });

            modelBuilder.Entity<SpecialtyImage>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Specialt__3214EC0723A4446A");

                entity.Property(e => e.SpecialtyId).IsRequired();
                entity.Property(e => e.ImageUrl).HasMaxLength(255);

                entity.HasOne(d => d.Specialty).WithMany(p => p.SpecialtyImages)
                    .HasForeignKey(d => d.SpecialtyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Specialty__Speci__6754599E");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Ratings__3214EC0734E80E58");

                entity.HasIndex(e => new { e.SpecialtyId, e.CreatedAt }, "IX_Ratings_SpecialtyId_CreatedAt").IsDescending(false, true);

                entity.Property(e => e.Stars).IsRequired();
                entity.Property(e => e.Comment);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
                entity.Property(e => e.UpdatedAt);

                entity.HasOne(d => d.Specialty).WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.SpecialtyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Ratings__Special__6D0D32F4");

                entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Ratings__UserId__6C190EBB");
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Ingredie__3214EC0710C3EF49");

                entity.HasIndex(e => e.Name, "UQ__Ingredie__737584F67D5CF1AE").IsUnique();

                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.NamePlain).HasMaxLength(100);
                entity.Property(e => e.Description);
                entity.Property(e => e.ImageUrl).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
                entity.Property(e => e.UpdatedAt);
                entity.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
            });           

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Recipes__3214EC0793125680");

                entity.HasIndex(e => e.CreatedAt, "IX_Recipes_CreatedAt").IsDescending();

                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.NamePlain).HasMaxLength(200);
                entity.Property(e => e.IsOriginal).HasDefaultValue(false).IsRequired();
                entity.Property(e => e.PrepareTime);
                entity.Property(e => e.CookingTime).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);                
                entity.Property(e => e.IsApproved).HasDefaultValue(false).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
                entity.Property(e => e.UpdatedAt);

                entity.HasOne(d => d.Specialty).WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.SpecialtyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Recipes__Special__7B5B524B");
            });

            modelBuilder.Entity<RecipeStep>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__RecipeSt__3214EC078E336CCD");

                entity.ToTable("RecipeStep");
                
                entity.Property(e => e.StepNumber).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.ImageUrl).HasMaxLength(255);

                entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeSteps)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__RecipeSte__Recip__7E37BEF6");
            });

            modelBuilder.Entity<RecipeIngredient>(entity =>
            {
                entity.HasKey(e => new { e.RecipeId, e.IngredientId });

                entity.ToTable("RecipeIngredient");

                entity.HasIndex(e => e.IngredientId, "IX_RecipeIngredient_IngredientId");
                entity.HasIndex(e => e.RecipeId, "IX_RecipeIngredient_RecipeId");

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)").IsRequired();
                entity.Property(e => e.Unit).HasMaxLength(50).IsRequired();

                entity.HasOne(d => d.Ingredient).WithMany(p => p.RecipeIngredients)
                    .HasForeignKey(d => d.IngredientId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__RecipeIng__Ingre__02FC7413");

                entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeIngredients)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade)   
                    .HasConstraintName("FK__RecipeIng__Recip__02084FDA");
            });

            modelBuilder.Entity<UserIngredient>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__UserIngr__3214EC07FDCBE32F");

                entity.ToTable("UserIngredient");

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)").IsRequired();
                entity.Property(e => e.Unit).HasMaxLength(50).IsRequired();

                entity.HasOne(d => d.Ingredient).WithMany(p => p.UserIngredients)
                    .HasForeignKey(d => d.IngredientId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK__UserIngre__Ingre__07C12930");

                entity.HasOne(d => d.User).WithMany(p => p.UserIngredients)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserIngre__UserI__06CD04F7");
            });

            modelBuilder.Entity<UserFavoriteRecipe>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__UserFavo__3214EC07EF504CAE");

                entity.HasIndex(e => e.UserId, "IX_UserFavoriteRecipes_UserId");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.HasOne(d => d.Recipe).WithMany(p => p.UserFavoriteRecipes)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserFavor__Recip__114A936A");

                entity.HasOne(d => d.User).WithMany(p => p.UserFavoriteRecipes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserFavor__UserI__10566F31");
            });         

            modelBuilder.Entity<UserViewHistory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__UserView__3214EC073CD4D359");

                entity.ToTable("UserViewHistory", t =>
                {
                    t.HasCheckConstraint("CK_UserViewHistory_SpecialtyOrRecipe",
                        "(SpecialtyId IS NOT NULL AND RecipeId IS NULL) OR (SpecialtyId IS NULL AND RecipeId IS NOT NULL)");
                });

                entity.HasIndex(e => e.RecipeId, "IX_UserViewHistory_RecipeId");
                entity.HasIndex(e => e.SpecialtyId, "IX_UserViewHistory_SpecialtyId");
                entity.HasIndex(e => e.UserId, "IX_UserViewHistory_UserId");

                entity.Property(e => e.ViewedAt).HasDefaultValueSql("(sysdatetime())").IsRequired();

                entity.HasOne(d => d.Recipe).WithMany(p => p.UserViewHistories)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserViewH__Recip__123EB7A3");

                entity.HasOne(d => d.Specialty).WithMany(p => p.UserViewHistories)
                    .HasForeignKey(d => d.SpecialtyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserViewH__Speci__114A936A");

                entity.HasOne(d => d.User).WithMany(p => p.UserViewHistories)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserViewH__UserI__10566F31");
            });

            // Indexes for performance & search
            modelBuilder.Entity<Specialty>()
                .HasIndex(d => d.Name);
            modelBuilder.Entity<Specialty>()
                .HasIndex(d => d.NamePlain);
            modelBuilder.Entity<Specialty>()
                .HasIndex(d => d.ProvinceId);

            modelBuilder.Entity<Ingredient>()
                .HasIndex(n => n.Name);
            modelBuilder.Entity<Ingredient>()
                .HasIndex(n => n.NamePlain);

            modelBuilder.Entity<Recipe>()
                .HasIndex(c => c.Name);
            modelBuilder.Entity<Recipe>()
                .HasIndex(c => c.NamePlain);
            modelBuilder.Entity<Recipe>()
                .HasIndex(c => c.SpecialtyId);

            modelBuilder.Entity<Province>()
                .HasIndex(t => t.Name);
            modelBuilder.Entity<Province>()
                .HasIndex(t => t.NamePlain);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.FullName);
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.City);
        }
    }
}
